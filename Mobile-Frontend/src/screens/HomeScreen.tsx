import React, { useEffect, useState, useCallback } from 'react';
import { View, Text, StyleSheet, FlatList, ActivityIndicator, SafeAreaView, RefreshControl } from 'react-native';
import { Plus } from 'lucide-react-native';
import EmployeeService, { Employee } from '../services/employeeService';
import EmployeeListItem from '../components/EmployeeListItem';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList } from '../../App';
import { useFocusEffect } from '@react-navigation/native';
import AppleTextInput from '../components/AppleTextInput';

type HomeScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Home'>;

interface Props {
  navigation: HomeScreenNavigationProp;
}

const HomeScreen: React.FC<Props> = ({ navigation }) => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');

  const fetchEmployees = async () => {
    try {
      const data = await EmployeeService.getAll();
      setEmployees(data);
    } catch (error) {
      console.error('Failed to fetch employees:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useFocusEffect(
    useCallback(() => {
      fetchEmployees();
    }, [])
  );

  const onRefresh = () => {
    setRefreshing(true);
    fetchEmployees();
  };

  const filteredEmployees = employees.filter((emp) =>
    `${emp.firstName} ${emp.lastName}`.toLowerCase().includes(searchQuery.toLowerCase()) ||
    emp.department?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  React.useLayoutEffect(() => {
    navigation.setOptions({
      headerRight: () => (
        <Plus
          size={28}
          color="#007AFF"
          onPress={() => navigation.navigate('EmployeeEdit', {})}
        />
      ),
    });
  }, [navigation]);

  if (loading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" color="#007AFF" />
      </View>
    );
  }

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.searchContainer}>
        <AppleTextInput
          placeholder="Search employees..."
          value={searchQuery}
          onChangeText={setSearchQuery}
          clearButtonMode="while-editing"
        />
      </View>
      <FlatList
        data={filteredEmployees}
        keyExtractor={(item) => item.id}
        renderItem={({ item }) => (
          <EmployeeListItem
            employee={item}
            onPress={() => navigation.navigate('EmployeeDetail', { id: item.id })}
          />
        )}
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor="#007AFF" />
        }
        contentContainerStyle={styles.listContent}
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Text style={styles.emptyText}>No employees found</Text>
          </View>
        }
      />
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F2F2F7',
  },
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  searchContainer: {
    paddingHorizontal: 16,
    paddingTop: 10,
    backgroundColor: '#F2F2F7',
  },
  listContent: {
    paddingBottom: 20,
  },
  emptyContainer: {
    padding: 32,
    alignItems: 'center',
  },
  emptyText: {
    fontSize: 16,
    color: '#8E8E93',
  },
});

export default HomeScreen;
